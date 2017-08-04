
#!/bin/sh
cd $TRAVIS_BUILD_DIR/src/spa/
sbt ++$TRAVIS_SCALA_VERSION package