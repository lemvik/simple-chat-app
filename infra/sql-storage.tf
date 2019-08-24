resource "azurerm_postgresql_server" "chat-app" {
  name                = "postgresql-server-1"
  location            = "${azurerm_resource_group.chat-app.location}"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"

  sku {
    name     = "B_Gen5_2"
    capacity = 2
    tier     = "Basic"
    family   = "Gen5"
  }

  storage_profile {
    storage_mb            = 5120
    backup_retention_days = 7
    geo_redundant_backup  = "Disabled"
  }

  administrator_login          = "psqladmin"
  administrator_login_password = "T0o$ecret7oo$ecure7oo4urious"
  version                      = "10"
  ssl_enforcement              = "Enabled"
}
