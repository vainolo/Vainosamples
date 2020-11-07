#include "WiFi.h"
#include "ESPAsyncWebServer.h"

#include <Arduino.h>

const char *ssid = "ESP32-Access-Point";
const char *password = "123456789";

AsyncWebServer server(80);

void setup()
{
  Serial.begin(115200);

  Serial.println("Setting AP (Access Point).");
  WiFi.softAP(ssid, password);   // Remove the password parameter, if you want the AP (Access Point) to be open

  IPAddress IP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(IP);

  server.on("/test", HTTP_GET, [](AsyncWebServerRequest *request) {
    Serial.println("Request received.");
    request->send_P(200, "text/plain", "Hello!");
  });

  server.begin();
}

void loop()
{
}