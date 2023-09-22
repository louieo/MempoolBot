# MempoolBot

Simple notifier of affordable transaction fee rates in a bitcoin mempool.

Queries the API of a bitcoin Mempool Space instance for recommended fee rates and notifies when the economy fee rate is below a configured sats/vByte threshold.

Supports email or Telegram.

Repeats the notification every x minutes as configured while under the threshold.