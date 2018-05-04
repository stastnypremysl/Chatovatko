#!/bin/bash
#Find folder, where is script saved
SOURCE="${BASH_SOURCE[0]}"
while [ -h "$SOURCE" ]; do # resolve $SOURCE until the file is no longer a symlink
DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"
SOURCE="$(readlink "$SOURCE")"
[[ $SOURCE != /* ]] && SOURCE="$DIR/$SOURCE" # if $SOURCE was a relative symlink, we need to resolve it relative to the path where the symlink file was located
      done
DIR="$( cd -P "$( dirname "$SOURCE" )" && pwd )"

#Run the file
cd /tmp
temp=$$Chat
mkdir $temp

cd $DIR/..
cd "Premy.Chatovatko"
cp -r Premy.Chatovatko.Server /tmp/$temp
cd /tmp/$temp/Premy.Chatovatko.Server
dotnet run;
cd /tmp
rm -r $temp

