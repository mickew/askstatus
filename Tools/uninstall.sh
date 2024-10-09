#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

   systemctl stop askstatusbackend.service
   systemctl disable askstatusbackend.service

   rm /etc/systemd/system/askstatusbackend.service

   rm -f -r /var/www/backend
   rm -f -r /var/www/frontend

   whiptail --title "Uninstall complete" --msgbox "Askstatus System uninstall complete." 8 78
fi
