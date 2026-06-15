# 🚀 Client Requirement Automation System

**Complete automated workflow for processing client requirements and sending professional HTML emails.**

---

## 📋 Project Overview

This project automates the entire client requirement processing workflow:

1. **Input**: Unstructured client requirement email (plain text)
2. **Processing**: Generate structured requirements using Gemini/Claude
3. **Conversion**: Transform markdown output into professional HTML email
4. **Output**: Send formatted email via Gmail

### File Structure

```
client-requirement-automation/
├── inputs/
│   ├── requirements.txt              # Raw client requirement email
│   └── Gemini_output.txt             # Markdown output from Gemini/Claude
├── outputs/
│   └── email_output.html             # Generated HTML email (for reference)
├── logs/
│   └── automation_YYYYMMDD_HHMMSS.log  # Execution logs
├── config/
│   └── SYSTEM_PROMPT.md              # LLM system prompt template
├── automate.py                       # Main automation script
└── README.md                         # This file
```

---

## 📝 Step-by-Step Setup Guide

### Step 1: Prerequisites

Ensure you have Python 3.7+ installed:

```bash
python3 --version
```

**Required libraries** (all built-in, no external dependencies):
- `smtplib` - Gmail SMTP integration
- `email` - Email message formatting
- `logging` - Script logging
- `pathlib` - File path management

### Step 2: Prepare Your Gmail Account

Gmail requires an **App Password** for automated email sending (not your regular password).

#### 2a. Enable 2-Step Verification

1. Go to [Google Account Security](https://myaccount.google.com/security)
2. In the left menu, click **Security**
3. Scroll down to "2-Step Verification"
4. Click **Get Started** and follow the steps
5. Verify your recovery email and phone number

#### 2b. Generate App Password

1. Return to [Google Account Security](https://myaccount.google.com/security)
2. Scroll down to **App passwords** (appears only after enabling 2-Step Verification)
3. Select:
   - App: **Mail**
   - Device: **Windows Computer** (or your device)
4. Click **Generate**
5. **Copy the 16-character password** (Google will show it once)

Example format: `abcd efgh ijkl mnop` (remove spaces when pasting)

### Step 3: Update Configuration

Edit `automate.py` and update the `Config` class with your Gmail details:

```python
class Config:
    # ... existing code ...
    
    # Email settings (MODIFY THESE)
    SENDER_EMAIL = "your-email@gmail.com"        # ← Your Gmail address
    SENDER_PASSWORD = "your-app-password"        # ← 16-char App Password (no spaces)
    RECIPIENT_EMAIL = "recipient@example.com"    # ← Target email address
```

**Example:**
```python
SENDER_EMAIL = "john.doe@gmail.com"
SENDER_PASSWORD = "abcdefghijklmnop"  # Without spaces
RECIPIENT_EMAIL = "team@company.com"
```

### Step 4: Prepare Input Files

#### 4a. Client Requirement Email (`requirements.txt`)

Place your unstructured client requirement email in `inputs/requirements.txt`:

```
Subject: Website Redesign Project - Requirements & Timeline

Hi there,

Hope you're having a great week! I need to get this website project rolling ASAP...
[rest of email]
```

A sample file is included for reference.

#### 4b. Generate Structured Requirements

You have two options:

**Option 1: Using Claude (Web UI)**
1. Go to [Claude.ai](https://claude.ai)
2. Copy the system prompt from `config/SYSTEM_PROMPT.md`
3. Paste it into a new Claude conversation
4. Copy your client email text and paste it after the prompt
5. Claude generates the structured requirements in markdown
6. Copy the entire response and save it to `inputs/Gemini_output.txt`

**Option 2: Using Gemini (Web UI)**
1. Go to [Google Gemini](https://gemini.google.com)
2. Copy the system prompt from `config/SYSTEM_PROMPT.md`
3. Paste it into a new Gemini conversation
4. Copy your client email text and paste it after the prompt
5. Gemini generates the structured requirements
6. Copy the entire response and save it to `inputs/Gemini_output.txt`

**Example of `Gemini_output.txt` structure:**

```markdown
# FUNCTIONAL REQUIREMENTS

## Must Have (MVP)

1. **Product Catalog with Filtering** (Medium Effort)
   - Filter by price, category, color, size
   - Search functionality
   - AC: Results load within 2 seconds

## Should Have

1. **Wishlist Feature** (Small Effort)
   - Users can save items for later
   - AC: Synced across sessions

# NON-FUNCTIONAL REQUIREMENTS

## Performance
- Homepage load time < 2 seconds on 4G
- Search results: < 500ms
- Lighthouse score ≥ 90

# TECHNICAL CONSTRAINTS
⚠️ ASSUMPTION: Stripe API integration already in place
⚠️ RISK: Warehouse system integration timeline unclear

# QUESTIONS FOR CLIENT
⏰ URGENT: Q: What's your exact budget range? — Why: Determines feature scope
```

---

## ⚙️ Execution Guide

### Running the Script

1. **Navigate to project directory:**
   ```bash
   cd /path/to/client-requirement-automation
   ```

2. **Run the script:**
   ```bash
   python3 automate.py
   ```

3. **Monitor execution:**
   - Console output shows real-time progress
   - Detailed logs saved to `logs/automation_YYYYMMDD_HHMMSS.log`

### Expected Output

**Console Output:**
```
2024-06-15 14:32:10,123 - INFO - ================================================================================
2024-06-15 14:32:10,124 - INFO - CLIENT REQUIREMENT AUTOMATION SCRIPT STARTED
2024-06-15 14:32:10,125 - INFO - Timestamp: 2024-06-15 14:32:10.125001
2024-06-15 14:32:10,126 - INFO - ================================================================================
2024-06-15 14:32:10,150 - INFO - 
2024-06-15 14:32:10,151 - INFO - 📌 STARTING AUTOMATED WORKFLOW
2024-06-15 14:32:10,152 - INFO - ================================================================================

[STEP 1] Reading input files...
✓ Successfully read requirements file: .../inputs/requirements.txt
✓ Successfully read Gemini output file: .../inputs/Gemini_output.txt

[STEP 2] Converting markdown to HTML...
✓ Successfully converted markdown to HTML

[STEP 3] Creating professional email template...
✓ Successfully created professional HTML email body

[STEP 4] Saving HTML output...
✓ HTML output saved to: .../outputs/email_output.html

[STEP 5] Sending email via Gmail...
Connecting to Gmail SMTP server (smtp.gmail.com:587)...
✓ SMTP connection established securely (TLS)
Authenticating with sender email: john.doe@gmail.com
✓ Authentication successful
Sending email to: team@company.com
✓ Email sent successfully to team@company.com
✓ SMTP connection closed

✅ WORKFLOW COMPLETED SUCCESSFULLY
```

**Generated Files:**
- `outputs/email_output.html` - Professional HTML email for reference
- `logs/automation_*.log` - Detailed execution log

---

## 🎨 Email Styling

The generated email features:

- **Professional Header** with gradient background (navy to blue)
- **Navy Blue Color Scheme** (#003366) for headers
- **Structured Sections** with clear typography
- **Inline CSS** for compatibility with all email clients
- **Responsive Design** that works on desktop and mobile
- **Code Block Styling** for technical requirements
- **Reference Section** showing original client email

### Customizing Colors

Edit the `Config` class to change styling:

```python
class Config:
    # ... existing code ...
    
    # Email styling constants
    HEADER_COLOR = "#003366"      # Main header color (navy)
    ACCENT_COLOR = "#0066CC"      # Accent color (bright blue)
    TEXT_COLOR = "#333333"        # Body text color
    BACKGROUND_COLOR = "#F5F5F5"  # Light backgrounds
    BORDER_COLOR = "#CCCCCC"      # Borders
```

---

## 🔧 Troubleshooting

### ❌ "File not found" Error

**Problem:** `FileNotFoundError: requirements.txt not found`

**Solution:**
1. Ensure both input files exist:
   - `inputs/requirements.txt`
   - `inputs/Gemini_output.txt`
2. Check that you're running the script from the correct directory
3. Verify file permissions (files should be readable)

```bash
# Verify files exist
ls -la inputs/
```

### ❌ Gmail Authentication Failed

**Problem:** `SMTPAuthenticationError: The application password is invalid`

**Solution:**
1. Verify you generated a Google App Password (not your Gmail password)
2. Remove any spaces from the password
3. Check 2-Step Verification is enabled:
   - Go to [Google Account Security](https://myaccount.google.com/security)
   - Verify "2-Step Verification" shows "On"
4. Try generating a new App Password

```python
# Check this is NOT your Gmail password
Config.SENDER_PASSWORD = "abcdefghijklmnop"  # ✓ Correct (16 chars)
# NOT this:
Config.SENDER_PASSWORD = "myGmailPassword123"  # ✗ Wrong
```

### ❌ SMTP Connection Timeout

**Problem:** `TimeoutError: [Errno 110] Connection timed out`

**Solution:**
1. Check your internet connection
2. Verify Gmail SMTP server is accessible:
   ```bash
   telnet smtp.gmail.com 587
   ```
3. Try running the script with less network interference
4. Check if your firewall is blocking SMTP (port 587)

### ❌ Email Sends But Formatting Looks Wrong

**Problem:** HTML styling doesn't display correctly in email client

**Solution:**
1. Some email clients (like Outlook) have limited CSS support
2. The script uses only inline CSS, which is widely supported
3. Try opening in a different email client
4. Check `outputs/email_output.html` in a web browser to see correct formatting

### ⚠️ Configuration Error

**Problem:** `SENDER_EMAIL not configured`

**Solution:**
1. Edit `automate.py`
2. Find the `Config` class
3. Update these three fields:
   ```python
   SENDER_EMAIL = "your-email@gmail.com"
   SENDER_PASSWORD = "your-app-password"
   RECIPIENT_EMAIL = "recipient@example.com"
   ```

---

## 📊 Log Files

Detailed logs are automatically saved to `logs/` directory.

**Log Format:**
```
2024-06-15 14:32:10,123 - INFO - ✓ Successfully read requirements file
2024-06-15 14:32:11,456 - INFO - [STEP 1] Reading input files...
2024-06-15 14:32:12,789 - ERROR - ✗ Error converting markdown to HTML: ...
```

**Access Logs:**
```bash
# View most recent log
tail -f logs/automation_*.log

# View entire log
cat logs/automation_20240615_143210.log
```

---

## 🔐 Security Considerations

### App Password Security

- ✅ **DO** store your App Password in `automate.py` on your local machine
- ❌ **DON'T** commit this file to public Git repositories
- ❌ **DON'T** share your App Password with others
- ✅ **DO** generate a new App Password if it's compromised

### Add to .gitignore

If using version control, prevent accidental commits:

```bash
# Create .gitignore in project root
echo "automate.py" >> .gitignore
echo "logs/" >> .gitignore
```

### Environment Variables (Advanced)

For production use, store credentials in environment variables:

```python
import os

class Config:
    SENDER_EMAIL = os.getenv('GMAIL_SENDER')
    SENDER_PASSWORD = os.getenv('GMAIL_APP_PASSWORD')
    RECIPIENT_EMAIL = os.getenv('GMAIL_RECIPIENT')
```

Then set in your shell:
```bash
export GMAIL_SENDER="your-email@gmail.com"
export GMAIL_APP_PASSWORD="abcdefghijklmnop"
export GMAIL_RECIPIENT="recipient@example.com"
```

---

## 📋 Workflow Checklist

Before running the script, verify:

- [ ] Python 3.7+ installed
- [ ] Gmail account with 2-Step Verification enabled
- [ ] App Password generated from Google Account
- [ ] `Config.SENDER_EMAIL` updated with your Gmail
- [ ] `Config.SENDER_PASSWORD` updated with App Password (no spaces)
- [ ] `Config.RECIPIENT_EMAIL` updated with target email
- [ ] `inputs/requirements.txt` contains client email
- [ ] `inputs/Gemini_output.txt` contains structured requirements (markdown)
- [ ] `inputs/` and `outputs/` directories exist

---

## 🎓 LLM System Prompt Usage

The system prompt in `config/SYSTEM_PROMPT.md` is designed to be:

1. **Copy-Pasteable**: Entire prompt fits in Claude/Gemini chat
2. **Comprehensive**: Covers all requirement analysis aspects
3. **Structured**: Clear sections for consistent output
4. **Professional**: Enterprise-grade requirement documentation

### Best Practices

1. **Use the entire prompt** - Don't remove sections
2. **Paste client email after prompt** - Let the LLM see full context
3. **Review output** - Check for missing or unclear requirements
4. **Ask follow-up questions** - If output is ambiguous
5. **Iterate** - Refine requirements with your team before automation

---

## 🚀 Advanced Usage

### Custom Email Subject

Modify the email subject line:

```python
send_email(email_body, subject_line="Custom Requirements Report - June 2024")
```

### Multiple Recipients

To send to multiple addresses, modify the script:

```python
# Change this:
message['To'] = Config.RECIPIENT_EMAIL

# To this:
recipients = ["email1@company.com", "email2@company.com"]
message['To'] = ", ".join(recipients)
server.sendmail(Config.SENDER_EMAIL, recipients, message.as_string())
```

### Custom CSS Styling

All CSS is in the `Config` class and email template. Modify colors:

```python
HEADER_COLOR = "#1a472a"      # Dark green
ACCENT_COLOR = "#2d5f3f"      # Forest green
TEXT_COLOR = "#2c3e50"        # Dark blue-gray
BACKGROUND_COLOR = "#ecf0f1"  # Light gray
BORDER_COLOR = "#95a5a6"      # Medium gray
```

---

## 📞 Support & Debugging

### Enable Debug Mode

Add to `automate.py` after imports:

```python
import logging
logging.basicConfig(level=logging.DEBUG)
```

### Check SMTP Configuration

Test Gmail SMTP connection:

```bash
# Install telnet if needed
# macOS: brew install telnet
# Ubuntu: sudo apt-get install telnet

telnet smtp.gmail.com 587
```

Expected output:
```
Trying 142.251.41.108...
Connected to smtp.gmail.com.
Escape character is '^]'.
220 smtp.google.com ESMTP ...
```

---

## 📝 Example Workflow

### Complete End-to-End Example

1. **Client sends requirement email** (unstructured)
   ```
   "Need new website with shopping cart, inventory sync, and payment integration. 
    Budget $50k, timeline 3 months. Must be fast and secure."
   ```

2. **Save to `inputs/requirements.txt`**

3. **Generate structured requirements** using Claude:
   - Copy `config/SYSTEM_PROMPT.md`
   - Paste into Claude chat
   - Paste client email after prompt
   - Copy Claude's response

4. **Save to `inputs/Gemini_output.txt`** (markdown format)

5. **Update `Config` class** with Gmail details

6. **Run script:**
   ```bash
   python3 automate.py
   ```

7. **Check output:**
   - Review `logs/automation_*.log` for execution details
   - Check email received at target address
   - Verify `outputs/email_output.html` in browser

---

## 📚 Additional Resources

- [Google App Passwords](https://support.google.com/accounts/answer/185833)
- [Gmail SMTP Configuration](https://support.google.com/mail/answer/7126229)
- [Python Email Documentation](https://docs.python.org/3/library/email.html)
- [SMTP Protocol (RFC 5321)](https://tools.ietf.org/html/rfc5321)

---

## 📄 License & Usage

This project is provided as-is for internal use. Feel free to modify and extend for your needs.

---

## ✨ Features Summary

| Feature | Details |
|---------|---------|
| **Input** | Plain text requirement email |
| **Processing** | LLM-generated structured requirements |
| **Output** | Professional HTML email with inline CSS |
| **Email Client** | Gmail via SMTP with TLS encryption |
| **Dependencies** | Python 3.7+ (built-in libraries only) |
| **Logging** | Detailed execution logs with timestamps |
| **Error Handling** | Comprehensive try-except blocks with helpful messages |
| **Security** | Google App Password authentication |
| **Customization** | Easy color and styling changes |
| **No External Libraries** | Pure Python stdlib (smtplib, email, logging, pathlib) |

---

**Happy automating! 🚀**

For questions or issues, review the Troubleshooting section or check the detailed log files in the `logs/` directory.
