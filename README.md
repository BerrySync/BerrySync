# This project is now DEPRECATED.
Goodberry's has revamped their calendar system to use Google Calendar directly rather than posting images. This project serves no longer serves any functional use.

# BerrySync
A program that syncs the Flavor of the Day calendar from [Goodberry's Flavor of the Day](https://www.goodberrys.com/flavor-of-the-day) with Google Calendar. It also includes a read-only API for finding information about flavors and the dates on which they occur. This project is still a WIP and more comprehensive documentation will be available in the future.

# How it works
Periodically, the updater crawls the Goodberry's website and downloads the calendars. These calendar images are pre-processed by cropping them into individual days and fed into Google's Vision AI, which returns the text on each image. The date and flavor information then gets inserted into the local database and added to Google Calendar.

# Usage
Navigate to a connected calendar and subscribe to it. The main instance of BerrySync syncs with [this](https://calendar.google.com/calendar/u/0?cid=aDR0NW5naHYzZXEwNGoxdWRxcHFnc3FlMTBAZ3JvdXAuY2FsZW5kYXIuZ29vZ2xlLmNvbQ).

# Setup
Obtain a key.json file for a service account from the [Google Cloud Console](https://console.cloud.google.com) and keep it somewhere safe. Make sure to copy the service account email. Next, enable [Google Vision](https://console.cloud.google.com/marketplace/product/google/vision.googleapis.com) and [Google Calendar](https://console.cloud.google.com/marketplace/product/google/calendar-json.googleapis.com) API access for your current project. Navigate to Google Calendar and create a calendar. Add the service account that was created earlier as a member that can make changes to events. Note the Calendar ID field in the calendar settings. Fill in the relevant information in the docker-compose file. Docker images TBA.
