#HAS NOW BEEN INTEGRATED INTO FRONTEND

# AQA assembly language emulator backend

this is the backend of my AQA reference language emulator
(gui to come)

## Author

- [@yetanotheregirl (emily)](https://github.com/yetAnotherEgirl)


## Documentation
the AQA assembly language instructionset can be found [here](https://filestore.aqa.org.uk/resources/computing/AQA-75162-75172-ALI.PDF)
#### Additional instructions:
by adding the preprossesor flag:
```
* EXTENDED
```
to the file you can use the extended instructionset
| instruction | |
| :- |:- |
| `INPUT Rn`| ask the user for an input and store it in Rn |
| `OUTPUT Rn` | output the value specified in register n to the user |
| `DUMP <storage>`| creates a .Dump file for the given storage type 

`<storage>` can be either `MEMORY`, `REGISTERS`, or `ALL`.

Assembly code can be split into multiple files, `assembly.aqa` will always be the starting point however adding the preprossesor flag:
```
* INSERT /path/to/file <where>
```
you can include other sections of assembly.

`<where>` can have 3 values:
| <where> | |
| :- | :- |
| `FIRST` | inserts the assembly at the begining of the file |
| `LAST`  | inserts the assembly at the end of the file |
| `HERE`  | inserts the assembly at the place of the flag |
