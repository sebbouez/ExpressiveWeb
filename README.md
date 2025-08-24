# Expressive Web

Expressive Web is a free software to build static websites using WYSIWYG blocks.  
It is written in .NET using Avalonia UI, so it should work correctly on the following systems: Windows, Linux, MacOS.  
But note that it has only been tested on Windows 11 for the moment.  
If you plan to compile it for other systems, please check Avalonia UI docs: https://docs.avaloniaui.net/docs/deployment/

## History

Expressive Web was created to be a successor of PageFabric, a free WYSIWYG website builder for Windows only written in
WPF.

## Vision

The project is in a very early stage of development.  
Differences with PageFabric are:

* No proprietary page format : use only pure HTML so you can edit them with another application if needed
* Not tied to one framework : use default one, Bootstrap,...
* Web references (aka packages in PageFabric) are no longer stored on proprietary servers, we now use JsDeliver, so you
  can download any package and use it already

The goal is to be able to work with several **kits** (Bootstrap,...).  
The user can use the visual designer to place predefined constructions or unit components.

### General features

| Feature                 | Status   | Comment                                        |
|-------------------------|----------|------------------------------------------------|
| Create new project      | On going |                                                |
| Open existing project   | Done     |                                                |
| Save file               | Done     |                                                |
| Work online/offline     | Done     |                                                |
| Multi tab system        | Done     |                                                |
| Multi language UI       | Done     |                                                |
| UI customization        | On going | Same system taken from PageFabric, own toolbar |
| Keyboard shortcuts      | On going | Code taken from Coho.Platform                  |
| Background activity     | Done     | Code taken from Coho.Platform                  |
| UI themes               | On going |                                                |
| UI components           | On going | Styling Avalonia UI components step by step    |
| UI Icons                | On going |                                                |
| Publish website locally |          |                                                |
| Publish website via FTP |          |                                                |
| Create directory        |          |                                                |
| Manage local files      |          |                                                |
| Versioning control      |          | Use Git, need to take code from PageFabric     |

### Designer features

| Feature                                    | Status   | Comment                                                                           |
|--------------------------------------------|----------|-----------------------------------------------------------------------------------|
| Undo/Redo process                          | Done     |                                                                                   |
| Adorner and decorations                    | Done     |                                                                                   |
| Basic DOM operations                       | Done     |                                                                                   |
| Move elements by drag drop                 | Done     |                                                                                   |
| Insert new html element                    | Done     |                                                                                   |
| Remove html element                        | Done     |                                                                                   |
| Basic text inline editing                  | Done     |                                                                                   |
| Filter html                                | Done     | Clean html when loading and saving files                                          |
| Page preview                               |          |                                                                                   |
| Page properties                            |          |                                                                                   |
| Menus management                           |          |                                                                                   |
| MasterPage/Partial pages                   |          |                                                                                   |
| Custom library of constructions            |          | Use your own constructions at several places (aka UserControl in many frameworks) |
| Styling per component                      |          |                                                                                   |
| Styling using styles library               |          | use CSS parser                                                                    |
| Javascript editor                          |          | + intellisense, link events to buttons...                                         |
| Tables editing                             |          |                                                                                   |
| Support components features                | On going | The foundation is here, need to work on the use cases                             |
| Quick actions in designer                  | On going | The foundation is here, need to work on the use cases                             |
| Custom components designer renderers       |          |                                                                                   |
| Load kit resources dynamically             |          |                                                                                   |
| Advanced text inline editing               |          | I plan to use ProseMirror for rich text editing                                   |
| Spell checking with multi linguage support |          | Need to port PageFabric code                                                      |
| Dictionary, synonyms                       |          | Need to port PageFabric code                                                      |
| Forms                                      |          |                                                                                   |
| Data binding using APIs                    |          |                                                                                   |

### Kits

| Feature                    | Status   | Comment |
|----------------------------|----------|---------|
| Default Kit                | On going |         |
| Bootstrap Kit              | On going |         |
| Flexible Components system | Done     |         |
| Style collection           | On going |         |

### Extensibility

Provide a way to extend the designer with custom components.  
Also be able to work with content (translate segment, replace image...).  
Open the path to AI extensions.
