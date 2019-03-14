# Dockerfile.build is a step, called with > docker build  -t main:build . -f Dockerfile.build  as part of a 2 stage build: 1) build the GO app, 2) load into NOOS container
# Note: this is a multi-stage build, requiring docker 17.05 or higher
########
#  Step 1 - build a binary
########
FROM golang:latest AS builder

# Install git (to fetch dependencies)
# RUN apk update && apk add --nocache git

# copy in source from project (main)
WORKDIR /go/src/main
COPY /src/main/. .

# Fetch dependencies
RUN go get -d -v

# Build the binary
#RUN go build -o /go/bin/main
RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o /go/bin/main

########
# Step 2 - build the image
########
# see documentation on scratch, https://hub.docker.com/_/scratch/ 
FROM scratch

# Copy out static executable
COPY --from=builder /go/bin/main /go/bin/main

# Run the main
ENTRYPOINT ["/go/bin/main"]