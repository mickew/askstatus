#!/usr/bin/env bash


CHOICE=$(
whiptail --title "Askstatus" --menu "Choose an option" 25 78 16 \
	"1)" "Install Askstatus on system." \
	"2)" "Update Askstatus on system." \
	"3)" "Remove Askstatus from the system." \
	"9)" "Exit."  3>&2 2>&1 1>&3	
)

case $CHOICE in
	"1)")   
		result="Install"
		echo "User selected $result."
		sh ./install.sh
	;;
	"2)")   
		result="Update"
		echo "User selected $result."
		sh ./update.sh
	;;

	"3)")   
		result="Uninstall"
		echo "User selected $result."
		if whiptail --title "Askstatus - Uninstall" --yesno "Do you realy want to uninstall" 8 78; then
			result="Uninstall"
			echo "User selected Yes."
			sh ./uninstall.sh
		else
			result=""
			echo "User selected No."
		fi
        ;;

	"9)") exit
        ;;
esac

whiptail --title "Askstatus - Complete" --msgbox "Askstatus System $result complete." 8 78
exit