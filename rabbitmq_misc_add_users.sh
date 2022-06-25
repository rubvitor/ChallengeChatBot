#!/bin/bash

create_additional_mq_users() {
        rabbitmqctl add_user "guest" "guest"
}
# add addtional users
create_additional_mq_users
