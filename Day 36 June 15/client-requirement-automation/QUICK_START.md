# ⚡ QUICK START GUIDE

**Get the automation running in 10 minutes.**

---

## 🎯 Prerequisites (2 minutes)

✅ **Have these ready:**
- Python 3.7+ installed (`python3 --version`)
- Gmail account with **2-Step Verification enabled**
- Generated **Google App Password** (16 characters)

❓ **Don't have these?** See "Setup Gmail" section below.

---

## 🔐 Setup Gmail (3 minutes)

### Step 1: Enable 2-Step Verification
1. Go to: https://myaccount.google.com/security
2. Click **"2-Step Verification"**
3. Follow prompts to add phone number
4. Click **"Get Started"**

### Step 2: Generate App Password
1. Return to: https://myaccount.google.com/security
2. Find **"App passwords"** at bottom
3. Select: App = **Mail** | Device = **Windows Computer**
4. Click **"Generate"**
5. **Copy the 16-character password** (shown once only)

Example: `abcd efgh ijkl mnop` → Use as: `abcdefghijklmnop`

---

## 🚀 Run in 5 Minutes

### Step 1: Update Configuration
Edit `automate.py` - Find this section (~line 30):

```python
class Config:
    # ... 
    SENDER_EMAIL = "your-email@gmail.com"        # ← Update
    SENDER_PASSWORD = "your-app-password"        # ← Update
    RECIPIENT_EMAIL = "recipient@example.com"    # ← Update
```

**Example:**
```python
SENDER_EMAIL = "john.doe@gmail.com"
SENDER_PASSWORD = "abcdefghijklmnop"
RECIPIENT_EMAIL = "boss@company.com"
```

### Step 2: Prepare Input Files

**File 1: `inputs/requirements.txt`**
- Already has sample client email
- Or replace with your own requirement email

**File 2: `inputs/Gemini_output.txt`**
Generate using Claude or Gemini:

1. Copy entire prompt from: `config/SYSTEM_PROMPT.md`
2. Go to [Claude.ai](https://claude.ai) or [Gemini.google.com](https://gemini.google.com)
3. Paste prompt
4. Paste your client email after prompt
5. Copy Claude/Gemini's response
6. Save to `inputs/Gemini_output.txt`

### Step 3: Run Script

```bash
# Navigate to project directory
cd /path/to/client-requirement-automation

# Run the script
python3 automate.py
```

**Expected output:**
```
📌 STARTING AUTOMATED WORKFLOW
✓ Successfully read requirements file
✓ Successfully read Gemini output file
✓ Successfully converted markdown to HTML
✓ Successfully created professional HTML email body
✓ HTML output saved to: outputs/email_output.html
✓ Email sent successfully to boss@company.com
✅ WORKFLOW COMPLETED SUCCESSFULLY
```

---

## ✅ Verify Success

Check these files:

1. **`outputs/email_output.html`**
   - Open in web browser
   - Verify styling looks professional

2. **`logs/automation_*.log`**
   - View execution details
   - Check for any warnings

3. **Check your email**
   - Should receive formatted email at `RECIPIENT_EMAIL`
   - HTML formatting should display correctly

---

## 🔧 Troubleshooting

| Error | Solution |
|-------|----------|
| `FileNotFoundError` | Make sure `inputs/requirements.txt` and `inputs/Gemini_output.txt` both exist |
| `SMTPAuthenticationError` | Verify App Password (not Gmail password). Check 2-Step Verification is enabled |
| `TimeoutError` | Check internet connection. Try again in a few moments |
| Configuration error | Update `Config` class with real email and App Password |

---

## 📁 File Structure

```
├── inputs/
│   ├── requirements.txt          ← Your client email
│   ├── Gemini_output.txt         ← LLM generated requirements
│   └── Gemini_output_SAMPLE.txt  ← Example (reference)
├── outputs/
│   └── email_output.html         ← Generated (opens in browser)
├── logs/
│   └── automation_*.log          ← Generated (execution details)
├── config/
│   ├── SYSTEM_PROMPT.md          ← Copy for Claude/Gemini
│   └── EMAIL_CONFIG_TEMPLATE.txt ← Configuration reference
├── automate.py                   ← Main script (UPDATE THIS)
└── README.md                     ← Full documentation
```

---

## 📊 Email Output Example

The script generates a professional HTML email with:

- ✨ Navy blue header with gradient
- 📋 Structured requirements sections
- 💼 Professional inline styling
- 📝 Original client email reference
- ✅ Ready to send or forward

---

## 🎓 Next Steps

1. ✅ Run the script successfully (you just did!)
2. 📧 Verify email received and formatted correctly
3. 📖 Read full `README.md` for advanced features
4. 🔄 Automate for multiple clients (copy workflow)

---

## 💡 Pro Tips

- **Save HTML locally**: Output HTML is saved to `outputs/` for reference
- **Check logs**: Detailed logs help debug issues
- **Customize colors**: Edit `HEADER_COLOR` etc in `Config` class
- **Multiple recipients**: Modify script to loop through email list
- **Schedule runs**: Use `cron` (macOS/Linux) or Task Scheduler (Windows)

---

## ⏱️ Typical Workflow Time

| Step | Time |
|------|------|
| Setup Gmail | 3 min |
| Update config | 1 min |
| Generate LLM output | 5 min |
| Run script | 30 sec |
| **Total** | **~10 min** |

---

## 🆘 Need Help?

- **Script errors?** Check `logs/automation_*.log`
- **Email issues?** Review "Troubleshooting" section above
- **Gmail setup?** See Google docs: https://support.google.com/accounts/answer/185833
- **Full guide?** Read `README.md` for comprehensive documentation

---

**You're all set! Happy automating! 🚀**
