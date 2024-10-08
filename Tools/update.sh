#!/usr/bin/env bash

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else
   BASEDIR=${PWD}
   systemctl stop askstatusbackend.service
   sh ./getlatest.sh

   chmod +x /var/www/backend/Askstatus.Web.API

   # create database folder
   mkdir -p /usr/share/askstatus
   sudo chmod 777 /usr/share/askstatus
   
   cd /var/www/backend/
   /var/www/backend/Askstatus.Web.API --seed
   cd ${BASEDIR}

   systemctl start askstatusbackend.service
   whiptail --title "Update complete" --msgbox "Askstatus System update complete." 8 78
fi
