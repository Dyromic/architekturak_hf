#!/bin/bash
# https://www.stuartellis.name/articles/shell-scripting/#enabling-better-error-handling-with-set
# set -Eeuo pipefail
# Based on mongo/docker-entrypoint.sh
# https://github.com/docker-library/mongo/blob/master/docker-entrypoint.sh#L303
if [ "$MONGO_DB_USERNAME" ] && [ "$MONGO_DB_PASSWORD" ]; then
    "${mongo[@]}" -u "$MONGO_ROOT_USERNAME" -p "$MONGO_ROOT_PASSWORD" --authenticationDatabase "$rootAuthDatabase" "$MONGO_DATABASE" <<-EOJS
        db.createCollection("files", {});
        db.createCollection("configs", {});
        db.createCollection("status", {});
        db.createCollection("users", {});
        db.createRole(
            {
                role: "user", 
                privileges: [
                    {
                        resource: { db: $(_js_escape "$MONGO_DATABASE"), collection: "files" },
                        actions: [ "find", "insert", "remove", "update" ]
                    },
                    {
                        resource: { db: $(_js_escape "$MONGO_DATABASE"), collection: "configs" },
                        actions: [ "find", "insert", "remove", "update" ]
                    },
                    {
                        resource: { db: $(_js_escape "$MONGO_DATABASE"), collection: "status" },
                        actions: [ "find", "insert", "remove", "update" ]
                    }
                ],
                roles: []
            }, {});
        db.createRole(
            {
                role: "auth", 
                privileges: [
                    {
                        resource: { db: $(_js_escape "$MONGO_DATABASE"), collection: "users" },
                        actions: [ "find", "insert", "remove", "update" ]
                    }
                ],
                roles: []
            }, {});
        db.createUser({
            user: $(_js_escape "$MONGO_DB_USERNAME"),
            pwd: $(_js_escape "$MONGO_DB_PASSWORD"),
            roles: [ { role: "user", db: $(_js_escape "$MONGO_DATABASE") } ]
        });
        db.createUser({
            user: $(_js_escape "$MONGO_AUTH_USERNAME"),
            pwd: $(_js_escape "$MONGO_AUTH_PASSWORD"),
            roles: [ { role: "auth", db: $(_js_escape "$MONGO_DATABASE") } ]
        });
EOJS
fi
