# Criaath Package

## Overview
Welcome to Criaath Package, your all-in-one solution for Unity projects! This package provides various functionalities to enhance your Unity development experience.

## Features
- **Basic and Advanced State Machine**: The basic version consists of Unity Events (State Enter, Update, Exit, OnStateChange) and is designed for simple tasks. The advanced version can be inherited and used for more complex tasks, such as within the included Entity system.
- **Audio System**: Play, pause, stop, set volume, and use methods like RandomPitch from any script or the Player component. Additionally, this system allows separate adjustment of Music and SFX levels when using suitable objects.
- **Criaath UI**: Consisting of Manager, Window, Page, and Commander components, this tool facilitates easy opening, closing, and transitioning between pages like Pause and Settings in games. It uses the "Animation-Sequencer" package by "brunomikoski" for animations during these transitions.
- **Out Of Screen (OOS)**: A tool for animating UI or world objects off-screen using "DOTween". This package, created by "demigiant," ensures smooth and visually appealing animations.

## Dependencies
Criaath Package includes several dependencies to ensure it functions properly. These dependencies are embedded within the package to streamline the installation process.

### Naughty Attributes
[Naughty Attributes](https://github.com/dbrizov/NaughtyAttributes) is a powerful Unity extension that enhances the inspector with various attributes and functionalities. This package is integrated seamlessly into Criaath Package to provide additional features and customization options. Note that Criaath Package includes a specific version of Naughty Attributes to ensure compatibility and stability. Modifying or updating Naughty Attributes independently may lead to unexpected behavior.

### Animation-Sequencer
[Animation-Sequencer](https://github.com/brunomikoski/Animation-Sequencer) by brunomikoski simplifies the process of animating UI transitions by allowing you to visualize and tweak animations without the need for recompilation. This tool is essential for the Criaath UI system and requires DOTween to function properly. Without DOTween, the Animation-Sequencer and Criaath UI system cannot be used.

### DOTween
[DOTween](http://dotween.demigiant.com/) by demigiant is a fast, efficient, and flexible tweening library for Unity. It allows for smooth and visually appealing animations, crucial for the functionality of the Out Of Screen tool and the Animation-Sequencer. To use these tools, DOTween must be installed in your project.

#### Installing DOTween
To install DOTween, follow these steps:
1. Download DOTween from the [official website](http://dotween.demigiant.com) or add it to your project via the Unity Asset Store.
2. Set up DOTween by clicking the "Setup DOTween" button in Unity.
3. In the Criaath Package, create the required "ASMDEF" files for tools that depend on DOTween by clicking the provided button.

By completing these steps, you'll ensure that all Criaath Package components function correctly and that you have access to its full range of features.

## Usage
Once Criaath Package is installed in your Unity project, you can start using its features right away. Refer to the documentation or examples provided with the package to learn how to leverage its functionalities effectively.

Happy developing!
