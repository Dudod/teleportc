## teleportc
# Test task for TeleportC.
I used simple implementation of Repository (didn't implement retry policy, didn't use transactions) and Tests (don't cover all layers of app) and I didn't use logging for save time. I used https://www.air-port-codes.com/ as external data provider, and free license are expiring on 14.06.

# Deploy
Just run `docker-compose up -d --build` App will be available at `http://localhost:5000/swagger` url.
