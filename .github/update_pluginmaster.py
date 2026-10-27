import json
import os
import sys
import time

repo_path = sys.argv[1]
version = sys.argv[2]
repo_full_name = sys.argv[3]

json_path = os.path.join(repo_path, 'pluginmaster.json')

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

download_url = f"https://github.com/{repo_full_name}/releases/download/v{version}/latest.zip"

entry = next((x for x in data if x.get('InternalName') == 'SomethingNeedDoing'), {})

entry['Name'] = 'Something Need Doing'
entry['Author'] = 'croizat'
entry['Punchline'] = 'Work work.'
entry['Description'] = 'Store unlimited macros and create lua scripts.'
entry['InternalName'] = 'SomethingNeedDoing'
entry['AssemblyVersion'] = version
entry['TestingAssemblyVersion'] = version
entry['DalamudApiLevel'] = 15
entry['TestingDalamudApiLevel'] = 15
entry['DownloadLinkInstall'] = download_url
entry['DownloadLinkUpdate'] = download_url
entry['DownloadLinkTesting'] = download_url
entry['RepoUrl'] = f'https://github.com/{repo_full_name}'
entry['IconUrl'] = 'https://s3.puni.sh/media/plugin/34/icon-z405dwrdjem.png'
entry['Tags'] = ['crafting', 'automation', 'lua', 'macros', 'scripting', 'SND']
entry['ApplicableVersion'] = 'any'
entry['LoadPriority'] = 0
entry['AcceptsFeedback'] = True
entry['LastUpdate'] = int(time.time())

if entry not in data:
    data.append(entry)

with open(json_path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=4, ensure_ascii=False)
