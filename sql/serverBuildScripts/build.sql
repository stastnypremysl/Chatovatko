START TRANSACTION;
CREATE SCHEMA IF NOT EXISTS `chatovatko` DEFAULT CHARACTER SET utf8 ;
/*----------------------------------------------------------------------*/
CREATE TABLE IF NOT EXISTS `chatovatko`.`users` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `public_certificate` MEDIUMTEXT NOT NULL,
  `public_certificate_sha2` VARBINARY(64) NOT NULL,
  `user_name` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `user_name_UNIQUE` (`user_name` ASC),
  UNIQUE INDEX `public_certificate_sha1_UNIQUE` (`public_certificate_sha2` ASC))
ENGINE = InnoDB;
/*----------------------------------------------------------------------*/
CREATE TABLE IF NOT EXISTS `chatovatko`.`blob_messages` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `recepient_id` INT NOT NULL,
  `sender_id` INT NOT NULL,
  `content` BLOB NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `recepiant_id` (`recepient_id` ASC, `id` ASC),
  INDEX `sender_id` (`sender_id` ASC),
  CONSTRAINT `recepient_id_foreign_key`
    FOREIGN KEY (`recepient_id`)
    REFERENCES `chatovatko`.`users` (`id`)
    ON DELETE CASCADE
    ON UPDATE RESTRICT,
  CONSTRAINT `sender_id_foreign_key`
    FOREIGN KEY (`sender_id`)
    REFERENCES `chatovatko`.`users` (`id`)
    ON DELETE CASCADE
    ON UPDATE RESTRICT)
ENGINE = InnoDB;
/*----------------------------------------------------------------------*/
CREATE TABLE IF NOT EXISTS `chatovatko`.`logs` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `message` MEDIUMTEXT NOT NULL,
  `class` VARCHAR(50) NOT NULL,
  `error` BIT(1) NOT NULL,
  `time_of_creation` DATETIME NOT NULL,
  `source` VARCHAR(45) NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC))
ENGINE = InnoDB;
/*----------------------------------------------------------------------*/
CREATE TABLE IF NOT EXISTS `chatovatko`.`users_keys` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `recepient_id` INT NOT NULL,
  `sender_id` INT NOT NULL,
  `aes_key` VARBINARY(1024) NOT NULL,
  `trusted` BIT(1) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `user_id_keys` (`recepient_id` ASC, `sender_id` ASC),
  INDEX `fk_public_certificates_users2_idx` (`sender_id` ASC),
  CONSTRAINT `fk_public_certificates_users1`
    FOREIGN KEY (`recepient_id`)
    REFERENCES `chatovatko`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_public_certificates_users2`
    FOREIGN KEY (`sender_id`)
    REFERENCES `chatovatko`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;
/*---------------------------------------------------------------------*/
CREATE TABLE IF NOT EXISTS `chatovatko`.`clients` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `user_id` INT NOT NULL,
  PRIMARY KEY (`id`),
  INDEX `fk_clients_users1_idx` (`user_id` ASC),
  CONSTRAINT `fk_clients_users1`
    FOREIGN KEY (`user_id`)
    REFERENCES `chatovatko`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;
COMMIT;