sudo docker-compose -f docker-compose.prod.yml down --remove-orphans --rmi "all";
sudo docker-compose -f docker-compose.prod.yml up --build;