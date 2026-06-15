# 🔑 GEMINI API SETUP GUIDE

**Get your free Gemini API key in 2 minutes.**

---

## ✅ Step 1: Access Google AI Studio

Go to: **https://aistudio.google.com/app/apikeys**

*(This is the official Google AI Studio - no sign-up needed if you have a Google account)*

---

## ✅ Step 2: Create API Key

### Option A: Create in New Project (Recommended)
1. Click the **"Create API Key"** button
2. Select **"Create new secret key in new project"**
3. Google creates a new project automatically
4. Your API key appears at the top

### Option B: Use Existing Project
1. Choose your existing Google Cloud project from dropdown
2. Click **"Create API Key"**
3. API key appears immediately

---

## ✅ Step 3: Copy Your API Key

The API key looks like:
```
AIzaSy...........................
```

**⚠️ IMPORTANT:**
- ✅ Copy the **full key** (it's very long)
- ✅ It displays **only once**
- ✅ Keep it safe (don't share publicly)
- ✅ You can regenerate it anytime if lost

---

## ✅ Step 4: Add to Configuration

Open `automate.py` and find line ~45:

```python
class Config:
    # ... other settings ...
    
    # Gemini API Configuration
    GEMINI_API_KEY = "AIzaSy..."  # ← PASTE YOUR KEY HERE
    GEMINI_MODEL = "gemini-1.5-flash"
```

**Replace** `"AIzaSy..."` with your actual API key:

```python
GEMINI_API_KEY = "AIzaSyDvlPKzz9z_XXXXXXXXX"  # Example (use your actual key)
```

---

## ✅ Verify Setup

Test your API key by running:

```bash
python3 automate.py
```

If working:
```
[STEP 2] Generating structured requirements with Gemini API...
Calling Gemini API: gemini-1.5-flash
✓ Successfully generated requirements from Gemini API
```

---

## 📊 Free Tier Limits

| Metric | Limit |
|--------|-------|
| **Requests/minute** | 15 |
| **Requests/day** | Unlimited |
| **Monthly cost** | FREE |
| **Model** | gemini-1.5-flash |

Perfect for:
- ✅ Processing 1-15 client emails per minute
- ✅ Daily batch processing
- ✅ Testing and development
- ✅ Small to medium projects

---

## 💰 Paid Tier (Optional)

If you need more than 15 req/min:

1. Go to: https://console.cloud.google.com/billing
2. Set up billing (credit card required)
3. Your quota automatically increases
4. Pay only for what you use

**Typical pricing:** ~$0.001 per request (very cheap!)

---

## 🔒 Security Best Practices

### ✅ DO
- ✅ Keep API key in `automate.py` locally only
- ✅ Use `.gitignore` to prevent commits
- ✅ Regenerate key if accidentally exposed
- ✅ Monitor API usage in Google Cloud Console

### ❌ DON'T
- ❌ Don't commit API key to GitHub
- ❌ Don't share API key with others
- ❌ Don't use in public codebases
- ❌ Don't post in forums/chat

### Add to .gitignore
Already included in this project:
```
# .gitignore
automate.py  # Contains your API key
```

---

## 🆘 Troubleshooting

### Error: "Invalid API Key"
- ✅ Solution: Verify you copied the full key (no spaces/truncation)
- ✅ Re-copy from Google AI Studio
- ✅ Check for extra spaces at beginning/end

### Error: "Rate Limit Exceeded"
- ✅ Solution: Free tier = 15 req/min
- ✅ Reduce request frequency
- ✅ Or upgrade to paid plan
- ✅ Wait ~1 minute and retry

### Error: "Access Denied / 403"
- ✅ Solution: Gemini API not enabled
- ✅ Go to: https://console.cloud.google.com/
- ✅ Enable "Generative Language API" in project
- ✅ Regenerate API key

### Error: "Key is invalid or doesn't exist"
- ✅ Solution: Regenerate new key from Google AI Studio
- ✅ Replace in automate.py
- ✅ Retry

### Still not working?
- ✅ Check internet connection
- ✅ Try from different network
- ✅ Check console logs for detailed error
- ✅ Verify Config.GEMINI_API_KEY is set

---

## 📱 Multiple Projects

### Using Same API Key for Multiple Scripts
```python
# automate.py
GEMINI_API_KEY = "AIzaSy..."  # Same key works for all projects

# other_project.py
GEMINI_API_KEY = "AIzaSy..."  # Can reuse same key
```

### Using Different Keys Per Project (Advanced)
```python
import os

GEMINI_API_KEY = os.getenv('GEMINI_API_KEY', 'AIzaSy...')
```

Then set environment variable:
```bash
export GEMINI_API_KEY="AIzaSy..."
python3 automate.py
```

---

## 🎓 Understanding the API Key

### What is an API Key?
- **Authentication credential** for Gemini API
- Like a password for the service
- Used to track your usage
- Enables billing (if paid)

### What Can It Do?
- ✅ Make requests to Gemini LLM
- ✅ Generate text, structured data, analysis
- ✅ Process up to your rate limit
- ✅ Tracks usage/billing

### What Can It NOT Do?
- ❌ Access your Google account
- ❌ Read your Gmail or files
- ❌ Modify your settings
- ❌ Access other Google services

---

## 📊 Monitoring Usage

### Check Your API Usage
1. Go to: https://console.cloud.google.com/
2. Select your project
3. Go to **"APIs & Services" → "Quotas"**
4. View Generative Language API usage

### What to Monitor
- Requests made (should match your usage)
- Quota remaining (if limited)
- API errors (should be 0)
- Response times

---

## 🚀 Ready to Go!

Your API key is set up and ready to use!

**Next steps:**
1. ✅ Paste API key in `automate.py`
2. ✅ Update Gmail credentials
3. ✅ Run `python3 automate.py`
4. ✅ Watch automation happen!

---

## 💡 Pro Tips

### Tip 1: Keep API Key Secure
```python
# Good - in local file only
GEMINI_API_KEY = "AIzaSy..."

# Not good - exposed in public GitHub
# Don't do this!
```

### Tip 2: Test API Connection
```bash
# Before processing client emails, verify:
python3 automate.py
# Should complete with "✅ FULL WORKFLOW COMPLETED SUCCESSFULLY"
```

### Tip 3: Monitor Costs
- Free tier: 15 requests/minute, completely free
- Paid tier: ~$0.001 per request (very cheap)
- Monitor usage in Google Cloud Console

### Tip 4: Batch Processing
```python
# Process multiple clients
for client in clients:
    requirements_file = f"inputs/{client}.txt"
    # script processes each one
```

---

## 🎉 You're All Set!

Your Gemini API is configured and ready to:
- ✅ Automatically analyze client emails
- ✅ Generate structured requirements
- ✅ Create professional HTML emails
- ✅ Send via Gmail

**No more manual copy-paste!** 🚀

---

**Questions?** Check the main README.md or run `python3 automate.py --help`
