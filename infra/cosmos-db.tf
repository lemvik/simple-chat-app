resource "azurerm_cosmosdb_account" "messages-db" {
  name                = "chat-app-account"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  offer_type          = "Standard"
  kind                = "MongoDB"

  enable_automatic_failover = false 

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 10
    max_staleness_prefix    = 200
  }

  geo_location {
    prefix            = "chat-app-account"
    location          = "${azurerm_resource_group.chat-app.location}"
    failover_priority = 0
  }
}
