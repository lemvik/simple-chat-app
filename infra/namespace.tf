resource "azurerm_servicebus_namespace" "chat-app" {
  name                = "chat-app-namespace"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}
