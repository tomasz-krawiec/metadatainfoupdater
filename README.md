# Metada Info Updater

## 1 Introduction

This is simple tool which generates insert scripts for ui.EnDtFieldDescription table in order to be picked by Data Dictionary tool and matched with contextName/dataKey for generated page. The acceptable format is just a generated DD doc with provided business descriptions/ field behaviors
1. **1.	Usage**
   - Compile sources and run the program
   - Make sure, that sheet containing DD updates is named: Sheet1 
   - First dialog window asks you to provide path to Excel doc with descriptions
   - Second dialog window asks you to provide output path for result scripts.

As we know, grids don’t have dataKey. In order to handle it ComponenUniqueNameMap class has grid name – datagrid name map, which is used while generating descriptions for them. Almost all of existing grids are included there, but if something is missing, or new grid is implemented, feel free to extend this mapping.

## 2 To do
   - refactor code to be able to merge desciprtions with updates for already existing pages
   - generate database column and database table info scripts
