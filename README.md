# Askholmen Status System
[![Build](https://github.com/mickew/askstatus/actions/workflows/build.yml/badge.svg)](https://github.com/mickew/askstatus/actions/workflows/build.yml)
[![Deploy](https://github.com/mickew/askstatus/actions/workflows/deploy.yml/badge.svg)](https://github.com/mickew/askstatus/actions/workflows/deploy.yml)
[![](https://img.shields.io/github/v/release/mickew/askstatus)](https://github.com/mickew/askstatus/releases/latest)
[![](https://img.shields.io/github/issues/mickew/askstatus)](https://github.com/mickew/askstatus/issues)
[![](https://img.shields.io/github/issues-closed/mickew/askstatus)](https://github.com/mickew/askstatus/issues?q=is%3Aissue+is%3Aclosed)
[![](https://img.shields.io/github/milestones/progress-percent/mickew/askstatus/2)](https://github.com/mickew/askstatus/milestone/2)
## Askholmen Status System

### Install, update and uninstall Askholmen Status System

```bash
bash <(curl -s https://raw.githubusercontent.com/mickew/askstatus/main/Tools/gettools.sh)
```

### linux commands

```bash
sudo systemctl status askstatusbackend.service
sudo systemctl stop askstatusbackend.service
sudo systemctl start askstatusbackend.service
journalctl -t askstatus-control -f
journalctl -t askstatus-control --since today
```

### Databese migrations

Add-Migration InitialCreate -OutputDir Data\Migrations

Update-Database

## Setup Raspbery PI
[Setup Raspbery PI for Askholmen Status System](Tools/RPISetup.md)
