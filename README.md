# MempoolBot

Simple email notifier of affordable transaction fee rates in a bitcoin mempool.

Queries the API of a bitcoin Mempool Space instance for recommended fee rates and sends an email when the economy fee rate is below a configured sats/vByte threshold.
Repeats the email every x minutes as configured while under the threshold.
