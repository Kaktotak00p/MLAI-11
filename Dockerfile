# Base image
FROM python:3.10

# Install system dependencies
RUN apt-get update && apt-get install -y \
    xvfb \
    wget \
    unzip \
    libglib2.0-0 \
    libsm6 \
    libxext6 \
    libxrender1 \
    git \
    sudo

# Set environment variables
ENV DISPLAY=:99

# Copy project files to the Docker container
COPY . /ml-agents
WORKDIR /ml-agents

# Install Python dependencies
RUN pip install --upgrade pip
RUN pip install -e ./ml-agents
RUN pip install -e ./ml-agents-envs
RUN pip install -r requirements.txt

# Expose the port for TensorBoard
EXPOSE 6006

# Entry point for the container
ENTRYPOINT ["xvfb-run", "python", "trainer.py"]

