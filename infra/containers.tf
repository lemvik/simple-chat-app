resource "azurerm_container_registry" "acr" {
  name                = "chatappregistry"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  location            = "${azurerm_resource_group.chat-app.location}"
  sku                 = "Basic"
  admin_enabled       = true 
}
