resource "azurerm_servicebus_topic" "chat-app" {
  name                = "chat-app-coordination-topic"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  namespace_name      = "${azurerm_servicebus_namespace.chat-app.name}"

  enable_partitioning = true
}

resource "azurerm_servicebus_subscription" "chat-app" {
  name                = "chat-app-coordination-subscription"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  namespace_name      = "${azurerm_servicebus_namespace.chat-app.name}"
  topic_name          = "${azurerm_servicebus_topic.chat-app.name}"
  max_delivery_count  = 1
  default_message_ttl = "PT19M5S"
}
