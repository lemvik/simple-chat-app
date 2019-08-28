resource "azurerm_logic_app_workflow" "chat-app-message-store" {
  name                = "chat-app-message-store"
  resource_group_name = "${azurerm_resource_group.chat-app.name}"
  location            = "${azurerm_resource_group.chat-app.location}"

  parameters {
    "$connections" = ""
  }
}

resource "azurerm_logic_app_trigger_custom" "topic-trigger" {
  name = "When_a_message_is_received_in_a_topic_subscription_(auto-complete)"
  logic_app_id = "${azurerm_logic_app_workflow.chat-app-message-store.id}"

  body = "{\"inputs\":{\"host\":{\"connection\":{\"name\":\"@parameters('$connections')['servicebus']['connectionId']\"}},\"method\":\"get\",\"path\":\"/@{encodeURIComponent(encodeURIComponent('chat-app-coordination-topic'))}/subscriptions/@{encodeURIComponent('chat-app-coordination-subscription')}/messages/head\",\"queries\":{\"subscriptionType\":\"Main\"}},\"recurrence\":{\"frequency\":\"Second\",\"interval\":15},\"type\":\"ApiConnection\"}"
}

resource "azurerm_logic_app_action_custom" "parse-json" {
  name = "Parse_JSON"
  logic_app_id = "${azurerm_logic_app_workflow.chat-app-message-store.id}"

  body = "{\"inputs\":{\"content\":\"@base64ToString(triggerBody()?['ContentData'])\",\"schema\":{\"properties\":{\"id\":{\"type\":\"string\"},\"m\":{\"type\":\"string\"},\"t\":{\"type\":\"integer\"},\"user\":{\"type\":\"string\"}},\"type\":\"object\"}},\"runAfter\":{},\"type\":\"ParseJson\"}"
}

resource "azurerm_logic_app_action_custom" "push-to-cosmos" {
  name = "Create_or_update_document"
  logic_app_id = "${azurerm_logic_app_workflow.chat-app-message-store.id}"

  body = "{\"inputs\":{\"body\":\"@base64ToString(triggerBody()?['ContentData'])\",\"headers\":{\"x-ms-documentdb-raw-partitionkey\":\"\\\"@{body('Parse_JSON')?['user']}\\\"\"},\"host\":{\"connection\":{\"name\":\"@parameters('$connections')['documentdb']['connectionId']\"}},\"method\":\"post\",\"path\":\"/dbs/@{encodeURIComponent('chat-app-database')}/colls/@{encodeURIComponent('chat-app-messages')}/docs\"},\"runAfter\":{\"Parse_JSON\":[\"Succeeded\"]},\"type\":\"ApiConnection\"}"
}
