#!/usr/bin/env python3
import os
import time
import requests

TOKEN = os.getenv('DONATIONALERTS_TOKEN')
QUEUE_PATH = os.getenv('DONATION_QUEUE_PATH')
if not QUEUE_PATH:
    base = os.getenv('RIMWORLD_CONFIG_PATH')
    if base:
        config_dir = os.path.join(base, 'RimConnect')
    else:
        config_dir = os.path.join(os.path.expanduser('~'), '.rimconnect_donations')
    QUEUE_PATH = os.path.join(config_dir, 'donations_queue.txt')
else:
    config_dir = os.path.dirname(QUEUE_PATH)

os.makedirs(config_dir, exist_ok=True)

LAST_ID = None

while True:
    if not TOKEN:
        print("DONATIONALERTS_TOKEN is not set")
        break
    try:
        resp = requests.get(
            'https://www.donationalerts.com/api/v1/alerts/donations',
            headers={'Authorization': f'Bearer {TOKEN}'},
            params={'limit': 1}
        )
        data = resp.json()
        if not data or 'data' not in data:
            time.sleep(5)
            continue
        donation = data['data'][0]
        donation_id = donation['id']
        if donation_id != LAST_ID:
            LAST_ID = donation_id
            amount = int(float(donation['amount']))
            username = donation.get('username', 'anon')
            with open(QUEUE_PATH, 'a', encoding='utf-8') as f:
                f.write(f"{amount};{username}\n")
    except Exception as e:
        print('Error fetching donations:', e)
    time.sleep(5)
