resource "azurerm_cosmosdb_account" "messages-db" {
  name                = "chat-app-account"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"
  is_virtual_network_filter_enabled = true

  enable_automatic_failover = false 

  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }

  lifecycle {
    ignore_changes = ["ip_range_filter"]
  }

  virtual_network_rule = [{id = "${azurerm_subnet.chat-app-subnet.id}"}]

  geo_location {
    location          = "${azurerm_resource_group.chat-app.location}"
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "chat-app-database" {
  name                = "chat-app-database"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  account_name        = "${azurerm_cosmosdb_account.messages-db.name}"
}

resource "azurerm_cosmosdb_sql_container" "chat-app-messages" {
  name = "chat-app-messages"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  account_name        = "${azurerm_cosmosdb_account.messages-db.name}"
  database_name       = "${azurerm_cosmosdb_sql_database.chat-app-database.name}"
  partition_key_path  = "/user"
}
