unset $(grep -v '^#' .env | sed -E 's/(.*)=.*/\1/' | xargs)
export $(grep -v '^#' .env | xargs)
