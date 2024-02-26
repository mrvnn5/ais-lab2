# Примеры использования
### Для запуска сервера  (/server)
`dotnet run <port> <max_threads> <save_path>`  
После запуска сервер принимает входящие соединения и записывает содержимое передаваемых сообщений в файл

### Для запуска клиента  (/client)
`dotnet run <path_to_file> <ip_addr> <port>`  
После запуска клиент отправляет содержимое файла серверу

# Инструкция по запуску
### Требования:  
OS: Ubuntu 22.04  
ПО: Git   
Созданная директория куда сервер будет сохранять файлы  
Пользователь root или из группы sudoers!  
Предварительно склонировать репозиторий: `git clone https://github.com/mrvnn5/ais-lab2.git`


## Для установки и старта демона запустить скрипт `install.sh`

Или
1. Установить dotnet    
`sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-7.0`  
2. Убедиться в установке    
`dotnet --version`  
3. Создать директорию для хранения файлов. Например    
`mkdir /var/files`  
4. Скопировать конфигурацию в systemd   
`cp server.service /etc/systemd/system/server.service`    
5. Обновить сервер   
`dotnet publish -c Release -o /srv/server`  
6. Запустить/перезапустить демон  
`sudo systemctl daemon-reload && sudo systemctl start server`  
7. Убедиться в работоспособности   
`sudo systemctl status server`
