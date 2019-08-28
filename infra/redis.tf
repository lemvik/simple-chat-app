resource "azurerm_redis_cache" "chat-app" {
  name                = "chat-app-cache"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  capacity            = 1
  family              = "C"
  sku_name            = "Standard"
  enable_non_ssl_port = false
  minimum_tls_version = "1.2"

  redis_configuration {}
}
