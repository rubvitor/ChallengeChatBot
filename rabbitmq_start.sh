#!/bin/bash

rabbitmq-server -detached
sleep 10
/usr/local/bin/rabbitmq_misc_set_queue_ttl.sh
/usr/local/bin/rabbitmq_misc_set_dlx_for_queues.sh
/usr/local/bin/rabbitmq_misc_add_users.sh
kill -9 $(ps aux | grep rabbitmq_server | grep erlang | awk '{print $2}') >/dev/null 2>&1
rabbitmq-server
