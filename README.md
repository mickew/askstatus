# Askholmen Status System
[![Build](https://github.com/mickew/askstatus/actions/workflows/build.yml/badge.svg)](https://github.com/mickew/askstatus/actions/workflows/build.yml)
[![Deploy](https://github.com/mickew/askstatus/actions/workflows/deploy.yml/badge.svg)](https://github.com/mickew/askstatus/actions/workflows/deploy.yml)
[![](https://img.shields.io/github/v/release/mickew/askstatus)](https://github.com/mickew/askstatus/releases/latest)
[![](https://img.shields.io/github/issues/mickew/askstatus)](https://github.com/mickew/askstatus/issues)
[![](https://img.shields.io/github/issues-closed/mickew/askstatus)](https://github.com/mickew/askstatus/issues?q=is%3Aissue+is%3Aclosed)
## Askholmen Status System

### Databese migrations

Add-Migration InitialCreate -OutputDir Data\Migrations

Update-Database

### linux commands

```bash
sudo systemctl status askstatusbackend.service
sudo systemctl stop askstatusbackend.service
sudo systemctl stop askstatusbackend.service
sudo journalctl -t askstatus-control --since today
```