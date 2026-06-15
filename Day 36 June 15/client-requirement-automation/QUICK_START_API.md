# ⚡ QUICK START - WITH GEMINI API AUTOMATION

**Fully automated: No manual copy-paste needed. Just run and go!**

---

## 🎯 Prerequisites (5 minutes)

✅ **Required:**
- Python 3.7+ installed (`python3 --version`)
- **Gemini API Key** (free, 2 minutes to get)
- Gmail account with **2-Step Verification enabled**
- **Google App Password** (16 characters)

---

## 🔑 Step 1: Get Gemini API Key (2 minutes)

### Get Free API Key
1. Go to: https://aistudio.google.com/app/apikeys
2. Click **"Create API Key"** button
3. Select **"Create new secret key in new project"**
4. **Copy the API key** (displayed once)
5. Keep it safe - you'll use it in Step 3

### Verify It Works
- The key looks like: `AIzaSy...` (very long string)
- Free tier includes 15 requests/minute
- Perfect for this automation

---

## 🔐 Step 2: Setup Gmail (3 minutes)

### Enable 2-Step Verification
1. Go to: https://myaccount.google.com/security
2. Click **"2-Step Verification"**
3. Follow prompts (add phone number)

### Generate App Password
1. Return to: https://myaccount.google.com/security
2. Find **"App passwords"** at bottom
3. Select: App = **Mail** | Device = **Windows Computer**
4. Click **"Generate"**
5. **Copy the 16-character password** (without spaces)

Example format: `abcdefghijklmnop`

---

## ⚙️ Step 3: Update Configuration (2 minutes)

Edit `automate.py` and find the `Config` class (~line 30):

```python
class Config:
    # ... other config ...
    
    # Gemini API Configuration (ADD THESE)
    GEMINI_API_KEY = "AIzaSy..."  # ← Paste your API key here
    
    # Gmail Configuration (EXISTING)
    SENDER_EMAIL = "your-email@gmail.com"
    SENDER_PASSWORD = "abcdefghijklmnop"  # 16-char app password
    RECIPIENT_EMAIL = "recipient@company.com"
```

**Update these 4 fields:**
```python
GEMINI_API_KEY = "paste-your-api-key-here"
SENDER_EMAIL = "your@gmail.com"
SENDER_PASSWORD = "16-char-app-password"
RECIPIENT_EMAIL = "target@email.com"
```

---

## 📝 Step 4: Prepare Client Email (1 minute)

Save your client requirement email to:
```
inputs/requirements.txt
```

**Example content:**
```
Subject: Website Redesign - Requirements

Hi, I need a new website with shopping cart, user accounts, and payment integration.
Budget: $50k, Timeline: 3 months. Must be fast and secure.

Thanks,
John
```

---

## 🚀 Step 5: Run the Script (30 seconds!)

```bash
cd /path/to/client-requirement-automation
python3 automate.py
```

**That's it!** The script will:

✅ Read your client email from `inputs/requirements.txt`  
✅ **Automatically generate** structured requirements using Gemini API  
✅ Convert to professional HTML email  
✅ Send via Gmail to your recipient  

---

## 📊 What Happens Automatically

```
Your Client Email
       ↓
python3 automate.py
       ↓
[Step 1] Read email
[Step 2] Call Gemini API → Generate requirements
[Step 3] Save requirements to Gemini_output.txt
[Step 4] Convert markdown → HTML
[Step 5] Create professional email
[Step 6] Save HTML to outputs/
[Step 7] Send via Gmail
       ↓
✅ Email Received!
```

---

## ✅ Verify Success

After running, check:

1. **Console Output**
   - Should show all 7 steps completed
   - Final message: `✅ FULL WORKFLOW COMPLETED SUCCESSFULLY`

2. **Generated Files**
   - `inputs/Gemini_output.txt` - Generated requirements
   - `outputs/email_output.html` - Professional HTML email
   - `logs/automation_*.log` - Execution details

3. **Email Received**
   - Check your recipient email inbox
   - Should have formatted requirements email

---

## 🔧 Troubleshooting

| Issue | Solution |
|-------|----------|
| `FileNotFoundError` | Create `inputs/requirements.txt` with client email |
| `GEMINI_API_KEY not configured` | Paste API key in Config class (step 3) |
| `Invalid API key` | Get new key from https://aistudio.google.com/app/apikeys |
| `SMTP Authentication failed` | Verify Gmail app password (not Gmail password) |
| `Rate limited` | Free tier allows 15 req/min. Wait a moment and retry |

---

## 🎯 Complete Workflow (Total Time)

| Step | Time | Action |
|------|------|--------|
| Get API Key | 2 min | Paste into Config |
| Setup Gmail | 3 min | Copy app password to Config |
| Update Config | 2 min | Fill 4 config fields |
| Prepare Email | 1 min | Save client email to inputs/ |
| Run Script | 30 sec | `python3 automate.py` |
| **Total** | **~8-9 min** | **Complete automation!** |

---

## 💡 What's Different from Manual Process?

### ❌ Old Way (Manual)
1. Copy system prompt from config/
2. Go to Claude web UI
3. Paste prompt + client email
4. Wait for response
5. Copy response
6. Save to file
7. Run script
**Total: ~20-30 minutes**

### ✅ New Way (Automated)
1. Run script
2. Done!
**Total: ~30 seconds**

**You save 20+ minutes per client! 🚀**

---

## 🎓 What Gets Generated

The Gemini API automatically creates:

```markdown
# FUNCTIONAL REQUIREMENTS
## Must Have
## Should Have
## Could Have

# NON-FUNCTIONAL REQUIREMENTS
## Performance
## Security
## Scalability

# TECHNICAL CONSTRAINTS & ASSUMPTIONS

# IDENTIFIED RISKS

# QUESTIONS FOR CLIENT
```

Perfect quality, professional format, **automatically generated**.

---

## 📂 Project Structure

```
client-requirement-automation/
├── automate.py                    ← Update Config section
├── QUICK_START_API.md             ← This file
├── README.md                      ← Full documentation
│
├── inputs/
│   └── requirements.txt           ← Paste client email here
│
├── outputs/
│   └── email_output.html          ← Generated HTML email
│
└── logs/
    └── automation_*.log           ← Execution logs
```

---

## 🚀 Advanced Options

### Custom System Prompt
Edit the `generate_requirements_with_gemini()` function in automate.py to customize the LLM behavior.

### Different Gemini Model
Change this line in Config:
```python
GEMINI_MODEL = "gemini-1.5-pro"  # Better quality (slower/costlier)
GEMINI_MODEL = "gemini-1.5-flash"  # Fast & cheap (default)
```

### Batch Processing
Create a simple loop:
```python
for client_email_file in emails:
    # Run automation for each client
    python3 automate.py
```

---

## 📊 Cost

**Completely Free!**

- ✅ Gemini API: Free tier (15 requests/min)
- ✅ Gmail: Your existing account
- ✅ Python: Open source
- ✅ Total cost: $0

---

## ❓ FAQ

**Q: Is my data secure?**
A: Yes. Data goes directly to Google's API. No third parties involved.

**Q: Can I use my own LLM?**
A: Yes, modify the `generate_requirements_with_gemini()` function to use any API.

**Q: What if API is down?**
A: The script will error gracefully. Check logs for details. Retry later.

**Q: Can I customize the output format?**
A: Yes, edit the `system_prompt` variable in `generate_requirements_with_gemini()`.

**Q: How many clients can I process?**
A: Unlimited! Free tier is 15 requests/minute, paid tier is higher.

---

## 🎉 You're Ready!

**Everything is automated. Just:**

1. ✅ Update 4 config fields
2. ✅ Save client email to inputs/requirements.txt
3. ✅ Run: `python3 automate.py`
4. ✅ Done!

No copy-paste, no manual steps, fully automated! 🚀

---

**Questions?** Check **README.md** for comprehensive documentation.
