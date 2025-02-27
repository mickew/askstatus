# Setup Raspbery PI for Askholmen Status System

## Install Raspberry PI OS
Installing Raspberry PI OS on a Raspberry PI is straight forward. Download the Raspberry PI Imager from [raspberrypi.org](https://www.raspberrypi.org/software/). Install the Raspberry PI Imager and use it to install Raspberry PI OS on a SD card.

## Setup Static IP
https://pimylifeup.com/raspberry-pi-static-ip-address/

## Setup Nginx as reverse proxy, Mosquitto MQTT boker and Certbot for SSL

```bash
sudo apt update -y && sudo apt upgrade -y

sudo apt-get install ufw

sudo ufw allow 22/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
sudo ufw allow 1883/tcp

sudo ufw enable

sudo apt install -y mosquitto mosquitto-clients
sudo systemctl enable mosquitto.service
sudo nano /etc/mosquitto/mosquitto.conf
add |listener 1883
add |allow_anonymous true
sudo systemctl restart mosquitto

sudo apt-get install nginx

bash <(curl -s https://raw.githubusercontent.com/mickew/askstatus/main/Tools/gettools.sh)

sudo nginx -s reload

sudo apt install snapd

sudo reboot

sudo snap install core; sudo snap refresh core

sudo snap install --classic certbot

sudo ln -s /snap/bin/certbot /usr/bin/certbot

sudo certbot --nginx
```