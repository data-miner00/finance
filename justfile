set shell := ["powershell.exe", "-c"]

dev:
    docker-compose up -d

prod:
    docker-compose -f docker-compose-prod.yml up -d