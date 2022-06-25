#!/bin/bash
if [ -n "$X_DEFAULT_ALL_QUEUES_TTL" ]; then
  if [[ $X_DEFAULT_ALL_QUEUES_TTL == ?(-)+([0-9]) ]]; then
    rabbitmqctl set_policy TTL ".*" '{"message-ttl":'$X_DEFAULT_ALL_QUEUES_TTL'}' --apply-to queues
  fi
fi

printenv| grep -oP "^X_TTL_\K.*" | tr '[:upper:]' '[:lower:]' | while read line; do
  q_name=$(echo $line | awk 'BEGIN {FS="="};{print $1}')
  q_ttl=$(echo $line | awk 'BEGIN {FS="="};{print $2}')

  rabbitmqctl set_policy TTL "$q_name.*" '{"dead-letter-exchange":"'$q_ttl'"}' --apply-to queues
done
