
#!/bin/sh
cd $TRAVIS_BUILD_DIR/src/spa/
sbt ++$TRAVIS_SCALA_VERSION package

# Steps to build react front end with web pack
# Build the container image from here as well.