sudo docker-compose down --remove-orphans --rmi "all";
sudo docker system prune -f;
sudo docker-compose up --build;