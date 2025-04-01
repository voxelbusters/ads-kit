# Made for Cross Platform Native Plugins 2.0 - Unity Asset Store ()
# by VoxelBusters Interactive LLP

#Usage
#> ./FindMethodCount.command FOLDER_NAME
#ex: ./FindMethodCount.command .
#dx executable can be found in AndroidSDKPath/build tools folder

echo "*****************************************************************************"
echo "finding method count for each jar"
echo "*****************************************************************************"

totalMethods=0
currentMethods=0

for i in $(find "$1" -name "*.jar")
do 
    dx --dex --output=temp.dex $i
    currentMethods=$(cat temp.dex | head -c 92 | tail -c 4 | hexdump -e '1/4 "%d "')
    printf "%-100s | %-10s \n" "$i" "$currentMethods"
    totalMethods=$((totalMethods+currentMethods))
done
rm temp.dex

echo "-----------------------------------------------------------------------------"
printf "%-100s | %-10s \n" "Total Method Count" "$totalMethods"
echo "-----------------------------------------------------------------------------"
