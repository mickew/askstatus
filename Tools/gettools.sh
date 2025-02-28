#!/usr/bin/env bash

getlatesturl="https://raw.githubusercontent.com/mickew/askstatus/main/Tools/getlatest.sh"
installurl="https://raw.githubusercontent.com/mickew/askstatus/main/Tools/install.sh"
uninstallurl="https://raw.githubusercontent.com/mickew/askstatus/main/Tools/uninstall.sh"
updateurl="https://raw.githubusercontent.com/mickew/askstatus/main/Tools/update.sh"
askstatusurl="https://raw.githubusercontent.com/mickew/askstatus/main/Tools/askstatus.sh"
askstatusservice="https://raw.githubusercontent.com/mickew/askstatus/main/Tools/askstatusbackend.service"

mkdir -p Tools
curl -o Tools/getlatest.sh $getlatesturl
curl -o Tools/install.sh $installurl
curl -o Tools/uninstall.sh $uninstallurl
curl -o Tools/update.sh $updateurl
curl -o Tools/askstatus.sh $askstatusurl
curl -o Tools/askstatusbackend.service $askstatusservice

cd Tools/
sudo sh askstatus.sh
