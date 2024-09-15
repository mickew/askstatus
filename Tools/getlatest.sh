#!/bin/bash

latesturl="https://github.com/mickew/askstatus/releases/latest"
rooturl="https://github.com/mickew/askstatus/releases/download"
filename="release.zip"
out="latest"

#Check if script is being run as root
if [ "$(id -u)" != "0" ]; then
   echo "This script must be run as root" 1>&2
   exit 1
fi

if [ ! $? = 0 ]; then
   exit 1
else

  rm -f $filename
  rm -f -r $out
  
  location=$(curl -s -I $latesturl | grep -i ^Location: | cut -d: -f2- | sed 's/^ *\(.*\).*/\1/')
  # printf "Redirect-url: %s\n\n" "$location"

  version="${location##*/}"
  # printf "Version: %s\n\n" "$version"

  ver=$(echo "$version" | sed 's/.\{1\}$//')
  # printf "Ver: %s\n\n" "$ver"

  url="${rooturl}/${ver}/${filename}"
  # printf "Url: %s\n\n" "$url"

  curl -L -O $url

  echo "unzipping downloaded files..."
  unzip -a -q -d $out/ $filename
  # unzip -a -d $out/ $filename

  rm -f ./$out/backend/appsettings.Development.json
  rm -f ./$out/frontend/appsettings.Development.json

  mkdir -p /var/www/backend
  mkdir -p /var/www/frontend

  sudo chmod 666 /var/www/backend
  sudo chmod 666 /var/www/frontend

  echo "Copying files..."
  cp -a ./$out/backend/. /var/www/backend 
  cp -a ./$out/frontend/. /var/www/frontend 

  echo "Cleaning up"
  rm -f $filename
  rm -f -r $out

fi
