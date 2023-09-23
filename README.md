# MempoolBot

Simple push notifier of transaction fee rates in a Bitcoin mempool.

Helps inform you when shitcoiners have run out of money and it may be a good time to make on-chain transactions that you have been waiting to do.  
Alternatively, just keep a track of fee rates periodically via a push notification.

.NET Core application written in C# and configured for deployment via Docker.

MempoolBot queries the API of a configured Bitcoin [Mempool Space](https://mempool.space/) instance for recommended fee rates and notifies when the "economy" fee rate is below a configured sats/vByte threshold.

The Mempool Space API conveniently calculates and returns fastestFee, halfHourFee, hourFee, economyFee and minimumFee.

Currently supports Email or Telegram as notification methods.

Repeats the notification every x minutes as configured while under the threshold.

The application can be configured to point to your own (or any) Bitcoin mempool (requires a self hosted Mempool Space API eg. via Umbrel or Start9)

## Installation (Docker)

- `git clone https://github.com/louieo/MempoolBot.git`
- If using Telegram method, follow Telegram Bot Setup below 
- Edit docker-compose.yml to set config (see Configuration below)
- Start container (see Usage below)

## Telegram Bot Setup (if using NotifyMethod Telegram)

- Login to Telegram with you own account
- Contact @BotFather and follow prompts to create a new bot and obtain a new token. This bot is under your control - don\'t share the token!
[Telegram Bot Tutorial](https://core.telegram.org/bots/tutorial)

## Configuration

- NotifyMethod: The method of notification. Must be Email or Telegram
- MempoolApiUrl: The URL of the Mempool Space API endpoint eg. http://myownnode:3006/api/v1 or http://mempool.space/api/v1
- EconomyRateThreshold: The "economy" fee rate of sats/vByte under which notifications will be triggered. eg. 10 
- NotifyRepeatFrequencyMinutes: Once the fee rate is under the threshold, how often the notification should be sent. eg. 30
- TelegramBotToken: The API token of the Telegram bot if NotifyMethod is Telegram. eg. 8611922966:AAG8aYA_G6bkwCudrTouiSLprK75NFGWnbE (fake\!)
- SmtpServer: The host of the SMTP server to use if NotifyMethod is Email. eg. smtp.gmail.com
- SmtpUser: The user to use when sending emails if NotifyMethod is Email. Usually a full email address. eg. test@gmail.com
- SmtpPass: The password to use when sending emails if NotifyMethod is Email. eg. dhsy lfjk utiy smap (fake example of Gmail auth code with 2FA on)
- FromEmail: The from email to send from if NotifyMethod is Email. eg. test@gmail.com
- ToEmail: The email to send notifications to if NotifyMethod is Email. eg. test@gmail.com

## Usage (Docker)

- Build and start the container with `docker compose up --build -d` (to run detached in background). Sometimes config changes are not picked up so --build forces a rebuild of the container
- Check the container is running with `docker ps`
- You can check the logs with `docker logs mempoolbot`
- Email method currently assumes SMTP over TLS/SSL port 587
- If using Telegram method, go to your bot on Telegram and click the Start button (or type /start)
  - If configured with the token correctly, Mempoolbot will receive a new chat and send some welcome info to the Telegram bot
  - After this, notifications will be sent when the fee rate goes under the configured threshold and repeat as configured
  - Most recent fees can be requested with /fees command
- The frequency of checking Mempool Space API is currently hard coded to 5 seconds
- If running in Docker, set the configuration via the environment variables in docker-compose.yml before building container
- If running standalone, set the configuration via appsettings.json

## Improvement Ideas

- See GitHub Issues
