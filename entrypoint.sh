#!/bin/bash

set -e
run_cmd_pull="docker pull rabbitmq:3-management"
run_cmd_run="docker run --rm -it -p 15672:15672 -p 5672:5672 rabbitmq:3-management"

>&2 echo "Pulling rabbitmq:3-management"
exec $run_cmd_pull

>&2 echo "Running rabbitmq:3-management"
exec $run_cmd_run