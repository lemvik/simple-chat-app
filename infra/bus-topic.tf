resource "azurerm_servicebus_topic" "chat-app" {
  name                = "chat-app-coordination-topic"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  namespace_name      = "${azurerm_servicebus_namespace.chat-app.name}"

  enable_partitioning = true
}
