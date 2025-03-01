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

   sh ./getlatest.sh

   chmod +x /var/www/backend/Askstatus.Web.API

   if [ ! -f /var/www/backend/appsettings.Production.json ]
   then
      sudo sh -c "echo '{}' >> /var/www/backend/appsettings.Production.json"
   fi

   sudo chown www-data:www-data /var/www/backend/appsettings.Production.json
   sudo chmod 644 /var/www/backend/appsettings.Production.json

   # create database folder
   mkdir -p /usr/share/askstatus
   sudo chmod 777 /usr/share/askstatus

   sudo chmod +x /var/www/backend/Askstatus.Web.API
   
   cd /var/www/backend/
   /var/www/backend/Askstatus.Web.API --seed
   cd ${BASEDIR}

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

   systemctl enable askstatusbackend.service
   systemctl start askstatusbackend.service
   whiptail --title "Installation complete" --msgbox "Askstatus System installation complete." 8 78

   #reboot
   #poweroff
fi
