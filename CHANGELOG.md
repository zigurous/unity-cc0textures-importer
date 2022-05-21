# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2022/05/20

### Added

- Added header label and instructions text to editor window
- Added "Browse Materials" button that links to the website

### Fixed

- New materials now use the default render pipeline shader instead of always assuming the built-in render pipeline
- Prevented non-texture files from being imported

## [1.1.0] - 2021/06/29

### Added

- Support for 16K textures

### Changed

- Links and references to new ambientCG.com domain

## [1.0.2] - 2021/04/13

### Fixed

- Set Editor assembly to only compile for the Editor platform

## [1.0.1] - 2021/03/07

### Changed

- Updated package metadata

## [1.0.0] - 2021/02/15

### Added

- Texture import
  - Resolutions: 1K, 2K, 4K, 8K
  - Formats: JPG, PNG
  - Maps: Albedo, Normal, Displacement, Roughness, Metallic, Occlusion, Emission
- Material import
