#!/bin/bash

X_DEFAULT_VHOST="/"
X_DEFAULT_PERMS="\".*\" \".*\" \".*\""
X_RABBITMQ_USERS_TAGS=('ADMINISTRATOR' 'MONITORING' 'POLICYMAKER' 'MANAGEMENT' 'IMPERSONATOR' 'NONE' 'guest')

check_x_rabbitmq_user_variables_exists() {
 local user=$1
 local x_password=X_RABBITMQ_USER_"$user"_PASSWORD
 local x_perms=X_RABBITMQ_USER_"$user"_PERMS
 local x_vhost=X_RABBITMQ_USER_"$user"_VHOST
 if [ ! -z "${!x_password}" ]; then
   if [  -z "${!x_vhost}" ]; then
     declare -g $x_vhost="$X_DEFAULT_VHOST"
   fi
   if [  -z "${!x_perms}" ]; then
     declare -g $x_perms="$X_DEFAULT_PERMS"
   fi
   return 0
 fi
 return 1
}

create_additional_mq_users() {
  for item in ${X_RABBITMQ_USERS_TAGS[*]}; do
    L_TAG=L_RABBITMQ_USERS_IN_"$item"
    X_TAG=X_RABBITMQ_USERS_IN_"$item"
    IFS=',' read -ra `echo ${L_TAG}` <<< "${!X_TAG}"
    tmp_array="${L_TAG}[@]"
    for uname in "${!tmp_array}"; do
      if check_x_rabbitmq_user_variables_exists $uname; then
        local x_password=X_RABBITMQ_USER_"$uname"_PASSWORD
        local x_perms=X_RABBITMQ_USER_"$uname"_PERMS
        local x_vhost=X_RABBITMQ_USER_"$uname"_VHOST
        local x_user_tag=$(echo $item| tr '[:upper:]' '[:lower:]')
        local x_user_name=$(echo $uname| tr '[:upper:]' '[:lower:]')

        rabbitmqctl add_user ${x_user_name} ${!x_password}
        rabbitmqctl set_permissions -p ${!x_vhost} ${x_user_name} ${!x_perms}
        if [ "$x_user_tag" != "none" ];then
          rabbitmqctl set_user_tags ${x_user_name} ${x_user_tag}
        fi
      fi
    done
  done
}
# add addtional users
create_additional_mq_users
