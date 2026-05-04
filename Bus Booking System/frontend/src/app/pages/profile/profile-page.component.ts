import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { ApiService } from '../../core/services/api.service';
import { UserProfile, UpdateProfileRequest } from '../../core/models';

@Component({
  selector: 'app-profile-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './profile-page.component.html'
})
export class ProfilePageComponent implements OnInit {
  profile: UserProfile | null = null;
  editMode = false;
  editData: UpdateProfileRequest = { name: '', mobileNo: '', gender: '', dob: '' };
  message = '';

  private readonly cdr = inject(ChangeDetectorRef);
  constructor(public readonly authService: AuthService, private readonly api: ApiService) {}

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.api.getProfile().subscribe({
      next: (data) => {
        console.log('[ProfilePage] loadProfile success', data);
        this.profile = data;
        this.editData = {
          name: data.name,
          mobileNo: data.mobileNo,
          gender: data.gender,
          dob: data.dob
        };
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Failed to load profile', err);
        this.profile = null;
        this.cdr.detectChanges();
      }
    });
  }

  toggleEdit() {
    this.editMode = !this.editMode;
    this.message = '';
  }

  saveProfile() {
    this.api.updateProfile(this.editData).subscribe({
      next: (res) => {
        this.message = 'Profile updated successfully!';
        this.editMode = false;
        this.loadProfile();
      },
      error: (err) => {
        this.message = 'Failed to update profile';
        this.cdr.detectChanges();
      }
    });
  }
}
