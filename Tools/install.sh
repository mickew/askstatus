#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

   sh ./getlatest.sh

   chmod +x /var/www/backend/Askstatus.Web.API

   # create database folder
   mkdir -p /usr/share/askstatus
   sudo chmod 777 /usr/share/askstatus

   sudo chmod +x /var/www/backend/Askstatus.Web.API
   
   /var/www/backend/Askstatus.Web.API --seed

   cp askstatusbackend.service /etc/systemd/system
   if [ ! -f /etc/systemd/system/askstatusbackend.service ]; then
     whiptail --title "Installation aborted" --msgbox "There was a problem writing the askstatusbackend.service file" 8 78
    exit
   fi

   # if [ ! -f /etc/nginx/sites-available/default.bak ]; then
   #   cp /etc/nginx/sites-available/default /etc/nginx/sites-available/default.bak
   # fi

   # cp default /etc/nginx/sites-available/default
   # if [ ! -f /etc/nginx/sites-available/default ]; then
   #   whiptail --title "Installation aborted" --msgbox "There was a problem writing the default nginx config file" 8 78
   #  exit
   # fi
   # nginx -s reload

   systemctl enable /etc/systemd/system/askstatusbackend.service
   systemctl start /etc/systemd/system/askstatusbackend.service
   whiptail --title "Installation complete" --msgbox "Askstatus System installation complete." 8 78

   #reboot
   #poweroff
fi
