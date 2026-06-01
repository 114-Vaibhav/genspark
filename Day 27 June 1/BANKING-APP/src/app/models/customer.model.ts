export class CustomerModel{
    username : string;
    name: string;
    email: string;
    
    phone: string;
    status: string;
    dateOfBirth: Date;

    constructor(username: string="", name: string="", email: string="", phone: string="", status: string="", dateOfBirth: Date= new Date()) {
        this.username = username;
        this.name = name;
        this.email = email;
        this.phone = phone;
        this.status = status;
        this.dateOfBirth = dateOfBirth;
    }

}