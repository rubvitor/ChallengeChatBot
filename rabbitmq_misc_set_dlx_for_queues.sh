#!/bin/bash
if [ -n "$X_DEFAULT_ALL_QUEUES_DLX" ]; then
  rabbitmqctl set_policy DLX ".*" '{"dead-letter-exchange":"'$X_DEFAULT_ALL_QUEUES_DLX'"}' --apply-to queues
fi


printenv| grep -oP "^X_DLX_\K.*" | tr '[:upper:]' '[:lower:]' | while read line; do
  q_name=$(echo $line | awk 'BEGIN {FS="="};{print $1}')
  q_dlx=$(echo $line | awk 'BEGIN {FS="="};{print $2}')

  rabbitmqctl set_policy DLX ".*" '{"dead-letter-exchange":"'$X_DEFAULT_ALL_QUEUES_DLX'"}' --apply-to queues
done
