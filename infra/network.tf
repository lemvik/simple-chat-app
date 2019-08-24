resource "azurerm_virtual_network" "chat-app" {
  name                = "chat-app-network"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  location            = "${azurerm_resource_group.chat-app.location}"
  address_space       = ["10.0.0.0/16"]
}
